import React, { Component } from 'react';
import { render } from 'react-dom';
import { UserRow, ManageInfo } from './Components/CommandersChildren';


const baseUri = "/admin/commanders";

export default class UserManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            users: null,
            roles: null,
            userIndex: null
        }
    }

    componentDidMount() { this.getData(); }

    getData() {
        $.ajax({
            type: 'get',
            url: `${baseUri}/data`,
        }).done((result) => {
            this.setState({
                users: result.fcs,
                roles: result.roles
            });
        }).fail((err) => {
            console.error(`React/Commanders {Commanders@getData} - Error getting active users`, err.responseText);
        });

        this.setUserIndex(null);
    }

    addGroup(role_id) {
        let account_id = (this.state.users[this.state.userIndex]) ? this.state.users[this.state.userIndex].id : null;
        let account_name = this.state.userInput;            
        $.ajax({
            type: 'post',
            url: `${baseUri}`,
            data: {
                "role_id": role_id,
                "account_id": account_id,
                "account_name": account_name
            }
        }).done(() => {
            let x = this.state.userIndex;
            this.getData();
            this.setUserIndex(x);
        }).fail((err) => {
            console.error(`React/Commanders {Commanders@addGroup} - Error adding group`, err.responseText);
        });
    }

    removeGroup(i) {    
        $.ajax({
            type: 'delete',
            url: `${baseUri}/revoke`,
            data: {
                roleId: i,
                accountId: this.state.users[this.state.userIndex].id || null
            }
        }).done((result) => {
            let x = this.state.userIndex;
            this.getData();
            this.setUserIndex(x);
        }).fail((err) => {
            console.error(`React/Commanders {Commanders@removeGroup} - Error revoking group`, err.responseText);
        });
    }

    setUserIndex(i) {
        this.setState({ userIndex: i });
    }

    getUsers() {
        return (!!this.state.users) ? this.state.users : null;
    }

    setStateFromInput(x) {
        this.setState({ userInput: x });
    }


    render() {
        let users;
        if (!!this.getUsers()) {
            users = this.getUsers().map((user, index) => {
                return <UserRow user={user} viewDetails={this.setUserIndex.bind(this)} index={index} key={index} />
            });
        }

        let userDetails;
        if (!!this.getUsers()) {
            userDetails = <ManageInfo details={this.state.users[this.state.userIndex]} roles={this.state.roles} onSubmit={this.addGroup.bind(this)} removeGroup={this.removeGroup.bind(this)} handleChange={this.setStateFromInput.bind(this)} reset={this.setUserIndex.bind(this)} />
        }

        return (
            <div className="container">
                <div className="row">
                    <div className="col-lg-8 col-md-12">
                        <table className="table table-responsive">
                            <thead>
                                <tr className="font-alpha">
                                    <th></th>
                                    <th>Name</th>
                                    <th>Corporation</th>
                                    <th>Alliance</th>
                                    <th>Roles</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {users}
                            </tbody>
                        </table>
                    </div>

                    <div className="col-lg-4 col-md-12">
                        {userDetails}
                    </div>
                </div>
            </div>
        )
    }
}



if (document.getElementById('user_management'))
    render(<UserManagement />, document.getElementById('user_management'));