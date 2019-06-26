import React, { Component } from 'react';
import { render } from 'react-dom';

import SpecialUsers from './Components/UserManagement/ElevatedUsers';
import UserInfo from './Components/UserManagement/UserInfo';

const baseUri = "/admin/commanders";

export default class UserManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            users: null,
            roles: null,
            userIndex: null,
            key: 0
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

    selectedUser() {
        if (this.state.users && this.state.users[this.state.userIndex])
            return this.state.users[this.state.userIndex];

        return null;
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
        return (
            <div className="container">
                <div className="row">
                    <div className="col-lg-8 col-md-12">
                        <SpecialUsers users={this.state.users}
                            u={this.getData.bind(this)}
                            setIndex={this.setUserIndex.bind(this)}
                            baseUri={baseUri} />
                        
                    </div>

                    <div className="col-lg-4 col-md-12">
                        <UserInfo
                            u={this.getData.bind(this)}
                            selectedUser={this.selectedUser()}
                            setIndex={this.setUserIndex.bind(this)}
                            userIndex={this.state.userIndex}
                            availableRoles={this.state.roles}
                            baseUri={baseUri}
                            key={this.state.key}  />
                    </div>
                </div>
            </div>
        )
    }
}



if (document.getElementById('user_management'))
    render(<UserManagement />, document.getElementById('user_management'));