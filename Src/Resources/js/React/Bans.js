import React, { Component } from 'react';
import { render } from 'react-dom';
import { BanRow, ManageInfo } from './Components/BansChildren';

export default class BanManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            bans: null,
            banIndex: null,
            baseUri: "/admin/bans"
        }
    }

    componentDidMount() {
        this.getData();
    }
    
    getData() {
        //Ajax call to API to get data
        $.ajax({
            type: 'get',
            url: `${this.state.baseUri}/active`,
            //headers: { 'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content') }
        }).done((activeBans) => {
            this.setState({
                bans: activeBans,
                banIndex: null
            });
        }).fail((err) => {
            console.error(`React/Bans {Bans@getData} - Error getting active bans`, err);
        })
    }

    getBans() {
        if(!!this.state.bans)
            return this.state.bans;

        return null;
    }

    setBanIndex(index) {
        this.setState({ banIndex: index});
    }

    submitForm(e) {
        e.PreventDefault();
        console.log(e);
    }

    // Revokes a ban by setting the 
    // timestamp for that ban to now.
    revokeBan(banId) {
        $.ajax({
            type: 'delete',
            url: `${this.state.baseUri}/revoke/${banId}`
        }).done(() => {
            this.getData();
        }).fail((err) => {
            console.error(`React/Bans {Bans@revokeBan} - Error revoking ban id: ${banId}`, err);
        });
    }

    render() {
        let bans;
        if (!!this.getBans()) {
            bans = this.getBans().map((ban, index) => {
                return <BanRow ban={ban} viewDetails={this.setBanIndex.bind(this)} revokeBan={this.revokeBan.bind(this)} index={index} key={index} />
            });
        }

        let banDetails = <ManageInfo onSubmit={this.submitForm.bind(this)} reset={this.setBanIndex.bind(this)}/>;
        if (this.state.bans != null && this.state.banIndex != null) {
            banDetails = <ManageInfo details={this.state.bans[0]} onSubmit={this.submitForm.bind(this)} reset={this.setBanIndex.bind(this)}/>
        }

        return (
            <div className="container">
                <div className="row">
                    <div className="col-lg-8 col-md-12">
                        <table className="bg-defuse table table-responsive">
                            <thead className="thead-inverse">
                                <tr className="font-alpha">
                                    <th></th>
                                    <th>Name</th>
                                    <th>Admin</th>
                                    <th></th>
                                    <th>Info</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {bans} 
                            </tbody>
                        </table>
                    </div>

                    <div className="col-lg-4 col-md-12">
                        {banDetails}
                    </div>
                </div>
            </div>
        );
    }
}

if (document.getElementById('bans'))
    render(<BanManagement />, document.getElementById('bans'));